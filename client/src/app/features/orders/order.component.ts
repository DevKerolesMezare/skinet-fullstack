import { Component, inject, OnInit, signal } from '@angular/core';
import { OrderService } from '../../core/services/order.service';
import { Order } from '../../shared/models/order';
import { RouterLink } from "@angular/router";
import { CurrencyPipe, DatePipe } from '@angular/common';

@Component({
  selector: 'app-order',
  imports: [RouterLink, DatePipe,CurrencyPipe],
  templateUrl: './order.component.html',
  styleUrl: './order.component.scss',
})
export class OrderComponent implements OnInit {
  private orderService = inject(OrderService)
  orders =signal<Order[]>([])

  ngOnInit() {
    this.orderService.getOrdersForUser().subscribe(orders => {
      this.orders.set(orders);
    });
  }

}
