import { Component, inject, input } from '@angular/core';
import { Product } from '../../../shared/models/Product';
import { MatCard, MatCardContent, MatCardActions } from '@angular/material/card';
import { CurrencyPipe } from '@angular/common';
import { MatAnchor } from "@angular/material/button";
import { MatIcon } from "@angular/material/icon";
import { RouterLink } from "@angular/router";
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-product-item',
  imports: [MatCard, MatCardContent, CurrencyPipe, MatCardActions, MatAnchor, MatIcon, RouterLink],
  templateUrl: './product-item.component.html',
  styleUrl: './product-item.component.scss',
})
export class ProductItemComponent {
  product = input<Product| null>(null);
  cartService = inject(CartService);


}
