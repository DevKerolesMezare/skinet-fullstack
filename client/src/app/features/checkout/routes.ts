import { Route } from '@angular/router';
import { CheckoutSuccessComponent } from './checkout-success/checkout-success.component';
import { authGuard } from '../../core/guards/auth-guard';
import { orderCompleteGuard } from '../../core/guards/order-complete-guard';
import { emptyCartGuard } from '../../core/guards/empty-cart-guard';
import { CheckoutComponent } from './checkout.component';

export const checkoutRoutes: Route[] = [
  { path: '', component: CheckoutComponent, canActivate: [authGuard, emptyCartGuard] },
  {
    path: 'success',
    component: CheckoutSuccessComponent,
    canActivate: [authGuard, orderCompleteGuard],
  },
];
