import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { DeliveryMethod } from '../../shared/models/deliveryMethod';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  deliveryMethod = signal<DeliveryMethod[] | null>(null);

  getDeliveryMethods() {
    if ((this.deliveryMethod()?.length ?? 0) > 0) {
      return of(this.deliveryMethod());
    }

    return this.http.get<DeliveryMethod[]>(this.baseUrl + 'payments/delivery-methods').pipe(
      map((methods) => {
        this.deliveryMethod.set(methods.sort((a, b) => b.price - a.price));
        return methods;
      }),
    );
  }
}
