import { Component, inject } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { Router, RouterLink } from '@angular/router';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { CartService } from '../../../core/services/cart.service';
import { CurrencyPipe } from '@angular/common';
import { SnackbarService } from '../../../core/services/snackbar.service';

@Component({
  selector: 'app-order-summary',
  imports: [MatButton, RouterLink, MatFormField, MatLabel, MatInput, CurrencyPipe],
  templateUrl: './order-summary.component.html',
  styleUrl: './order-summary.component.scss',
})
export class OrderSummaryComponent {
  cartService = inject(CartService);
  router = inject(Router);
  snack = inject(SnackbarService);

  // checkout() {
  //   if ((this.cartService.totals()?.total ?? 0) <= 0) {
  //     this.snack.error('Your cart is empty')
  //     return;
  //   }
  //   this.router.navigate(['/checkout']);
  // }
}
