import { Component, inject, OnInit, signal } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCardModule } from '@angular/material/card';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from '../../../shared/pipes/address-pipe';
import { PaymentCardPipe } from '../../../shared/pipes/payment-card-pipe';
import {  MatButton } from "@angular/material/button";


@Component({
  selector: 'app-order-detailed',
  imports: [
    MatCardModule,
    MatButton,
    AddressPipe,
    PaymentCardPipe,
    DatePipe,
    CurrencyPipe,
    RouterLink
],
  templateUrl: './order-detailed.component.html',
  styleUrl: './order-detailed.component.scss',
})
export class OrderDetailedComponent implements OnInit {
  private orderService = inject(OrderService);
  private activatedRoute = inject(ActivatedRoute);
  order = signal<Order | null>(null);

  ngOnInit() {
    this.loadOrder();
  }

  loadOrder() {
    const orderId = this.activatedRoute.snapshot.paramMap.get('id');
    if (orderId) {
      this.orderService.getOrderById(+orderId).subscribe(order => {
        this.order.set(order);
      });
    }
  }
}
