import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Order, OrderToCreate } from '../../shared/models/order';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  orderComplete = signal(false);

  createOrder(orderToCreate: OrderToCreate) {
    return this.http.post<Order>(`${this.baseUrl}orders`, orderToCreate);
  }

  getOrdersForUser() {
    return this.http.get<Order[]>(`${this.baseUrl}orders`);
  }

  getOrderById(id: number) {
    return this.http.get<Order>(`${this.baseUrl}orders/${id}`);
  }
}
