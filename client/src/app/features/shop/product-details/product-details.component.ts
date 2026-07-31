import { Component, inject, OnInit, signal } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../../shared/models/Product';
import { CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatFormField, MatLabel } from "@angular/material/form-field";
import { MatInput } from "@angular/material/input";
import { MatDivider } from "@angular/material/divider";
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: 'app-product-details',
  imports: [CurrencyPipe, MatButton, MatInput, MatFormField, MatLabel, MatDivider, MatIcon],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
})
export class ProductDetailsComponent implements OnInit {
  private shopService = inject(ShopService);
  private activatedRoute = inject(ActivatedRoute);
  product = signal<Product | null>(null);

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    if (!id) return;

    this.shopService.getProduct(+id).subscribe({
      next: (product) => this.product.set(product),
      error: (err) => console.log(err),
    });
  }
}
