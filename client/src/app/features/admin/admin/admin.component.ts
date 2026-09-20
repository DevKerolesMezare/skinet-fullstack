import { AfterViewInit, Component, inject, OnInit, signal, ViewChild } from '@angular/core';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Order } from '../../../shared/models/order';
import { AdminService } from '../../../core/services/admin.service';
import { OrderParams } from '../../../shared/models/orderParams';
import { MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatSelectChange, MatSelectModule } from '@angular/material/select';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatTabsModule, MatTab, MatTabGroup } from '@angular/material/tabs';
import { RouterLink } from '@angular/router';
import { DialogService } from '../../../core/services/dialog.service';

@Component({
  selector: 'app-admin',
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatIcon,
    MatSelectModule,
    DatePipe,
    CurrencyPipe,
    MatTooltipModule,
    MatTabsModule,
    MatIconButton,
    RouterLink,
  ],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss',
})
export class AdminComponent implements OnInit {
  displayedColumns = ['id', 'buyerEmail', 'OrderDate', 'total', 'status', 'action'];

  dataSource = new MatTableDataSource<Order>([]);
  private adminService = inject(AdminService);
  private dialogService = inject(DialogService);
  orderParams = signal(new OrderParams());
  totalItems = signal(0);
  statusOptions = ['All', 'PaymentReceived', 'PaymentMismatch', 'Refunded', 'Pending'];

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders() {
    this.adminService.getOrders(this.orderParams()).subscribe({
      next: (response) => {
        if (response.data) {
          this.dataSource.data = response.data;
          this.totalItems.set(response.count);
        }
      },
    });
  }

  onPageChange(event: PageEvent) {
    this.orderParams.update((params) => ({
      ...params,
      pageNumber: event.pageIndex + 1,
      pageSize: event.pageSize,
    }));
    this.loadOrders();
  }

  onFilterSelect(event: MatSelectChange) {
    this.orderParams.update((params) => ({
      ...params,
      filter: event.value,
      pageNumber: 1,
    }));
    this.loadOrders();
  }

  async openConfirmDialog(id: number) {
    const confirmed = await this.dialogService.confirm(
      'Cofirm refund',
      'Are you sure you want to issue this refund? This cannot be undone',
    );
  }

  refundOrder(id: number) {
    this.adminService.refundOrder(id).subscribe({
      next: (order) => {
        this.dataSource.data = this.dataSource.data.map((o) => (o.id === id ? order : o));
      },
    });
  }
}
