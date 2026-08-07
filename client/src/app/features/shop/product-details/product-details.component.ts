import { Component, inject, OnInit, signal } from '@angular/core';
import { ShopService } from '../../../core/services/shop.service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../../shared/models/Product';
import { CurrencyPipe } from '@angular/common';
import { MatButton } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatDivider } from '@angular/material/divider';
import { MatIcon } from '@angular/material/icon';
import { CartService } from '../../../core/services/cart.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-details',
  imports: [
    CurrencyPipe,
    MatButton,
    MatInput,
    MatFormField,
    MatLabel,
    MatDivider,
    MatIcon,
    FormsModule,
  ],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss',
})
export class ProductDetailsComponent implements OnInit {
  private shopService = inject(ShopService);
  private activatedRoute = inject(ActivatedRoute);
  cartService = inject(CartService);
  product = signal<Product | null>(null);
  quantityInCart = signal(0);
  quantity = signal(1);

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');

    if (!id) return;

    this.shopService.getProduct(+id).subscribe({
      next: (product) => {
        this.product.set(product);
        this.updateQuantityInCart();
      },
      error: (err) => console.log(err),
    });
  }
  updateCart() {
    const product = this.product();
    if (!product) return;

    if (this.quantity() > this.quantityInCart()) {
      const itemToAdd = this.quantity() - this.quantityInCart();
      
      this.quantityInCart.update((value) => value + itemToAdd);
      this.cartService.addItemToCart(product, itemToAdd);
    } else {
      const itemsToRemove = this.quantityInCart() - this.quantity();

      this.quantityInCart.update((value) => value - itemsToRemove);
      this.cartService.removeItemFromCart(product.id, itemsToRemove);
    }
  }
  updateQuantityInCart() {
    this.quantityInCart.set(
      this.cartService.cart()?.items.find((x) => x.productId === this.product()?.id)?.quantity || 0,
    );

    this.quantity.set(this.quantityInCart() || 1);
  }

  getButtonText() {
    return this.quantityInCart() > 0 ? 'Update cart' : 'Add to cart';
  }
}
