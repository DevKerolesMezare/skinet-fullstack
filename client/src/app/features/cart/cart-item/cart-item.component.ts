import { Component, inject, input } from '@angular/core';
import { CartItem } from '../../../shared/models/cart';
import { RouterLink } from '@angular/router';
import { MatIcon } from '@angular/material/icon';
import { CurrencyPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-cart-item',
  imports: [RouterLink, MatIcon, CurrencyPipe, MatButtonModule],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss',
})
export class CartItemComponent {
  item = input.required<CartItem>();
  cartservice = inject(CartService);

  incrementQuantity() {
    this.cartservice.addItemToCart(this.item());
  }

  decrementQuantity() {
    this.cartservice.removeItemFromCart(this.item().productId);
  }

  removeItemFromCart() {
    this.cartservice.removeItemFromCart(this.item().productId, this.item().quantity);
  }
}
