import { Component, inject, OnInit, signal } from '@angular/core';
import { ShopService } from '../../core/services/shop.service';
import { Pagination, Product } from '../../shared/models/Product';
import { ProductItemComponent } from './product-item/product-item.component';
import { MatDialog } from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatListOption, MatSelectionList, MatSelectionListChange } from '@angular/material/list';
import { ShopParams } from '../../shared/models/shopParams';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-shop',
  imports: [
    ProductItemComponent,
    MatButton,
    MatIcon,
    MatMenu,
    MatSelectionList,
    MatListOption,
    MatMenuTrigger,
    MatPaginator,
    FormsModule,
    MatIconButton
],
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss'],
})
export class ShopComponent implements OnInit {
  private shopService = inject(ShopService);
  private dialogService = inject(MatDialog);
  products = signal<Pagination<Product> | null>(null);

  sortOptions = [
    { name: 'Alphabetical', value: 'name' },
    { name: 'Price: Low to High', value: 'PriceAsc' },
    { name: 'Price: High to Low', value: 'PriceDesc' },
  ];

  shopParams = signal(new ShopParams());
  pageSizeOptions = [5, 10, 15, 20];

  ngOnInit(): void {
    this.initializeShop();
  }

  initializeShop() {
    this.shopService.getBrands();
    this.shopService.getTypes();
    this.getProducts();
  }

  getProducts() {
    this.shopService.getProducts(this.shopParams()).subscribe({
      next: (res) => {
        this.products.set(res);
      },
      error: (er) => console.log(er),
    });
  }

  onSearchChange(value: string) {
    this.shopParams.update((params) => ({
      ...params,
      search: value,
      pageNumber: 1,
    }));

    this.getProducts();
  }

  handlePageEvent(event: PageEvent) {
    this.shopParams.update((params) => ({
      ...params,
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize,
    }));

    this.getProducts();
  }

  onSortChange(event: MatSelectionListChange) {
    const selectedOption = event.options[0];

    if (selectedOption) {
      this.shopParams.update((params) => ({
        ...params,
        sort: selectedOption.value,
        pageNumber: 1,
      }));

      this.getProducts();
    }
  }

  openFilterDialog() {
    const dialogRef = this.dialogService.open(FiltersDialogComponent, {
      minWidth: '500px',
      data: {
        selectedBrands: this.shopParams().brands,
        selectedTypes: this.shopParams().types,
      },
    });

    dialogRef.afterClosed().subscribe({
      next: (result) => {
        if (result) {
          this.shopParams.update((params) => ({
            ...params,
            brands: result.selectedBrands,
            types: result.selectedTypes,
            pageNumber: 1,
          }));

          this.getProducts();
        }
      },
      error: (err) => console.log(err),
    });
  }
}
