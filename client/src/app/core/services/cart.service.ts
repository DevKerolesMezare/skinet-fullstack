import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Cart, CartItem } from '../../shared/models/cart';
import { Product } from '../../shared/models/Product';
import { map } from 'rxjs/internal/operators/map';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  baserUrl = environment.apiUrl;
  private http = inject(HttpClient);
  cart = signal<Cart | null>(null);
  itemCount = computed(() => {
    return this.cart()?.items.reduce((count, item) => count + item.quantity, 0) ?? 0;
  });

  totals = computed(() => {
    const cart = this.cart();
    if (!cart) return null;
    const subtotal = cart.items.reduce((sum, item) => sum + item.price * item.quantity, 0);
    const shipping = 0;
    const discount = 0;
    return {
      subtotal,
      shipping,
      discount,
      total: subtotal + shipping - discount,
    };
  });

  getCart(id: string) {
    return this.http.get<Cart>(this.baserUrl + 'cart?id=' + id).pipe(
      map((cart) => {
        this.cart.set(cart);
        return cart;
      }),
    );
  }

  setCart(cart: Cart) {
    return this.http.post<Cart>(this.baserUrl + 'cart', cart).subscribe({
      next: (cart) => {
        this.cart.set(cart);
      },
      error: (error) => {
        console.log(error);
      },
    });
  }

  addItemToCart(item: CartItem | Product, quantity = 1) {
    const currentCart = this.cart() ?? this.CreateCart();
    if (this.isProduct(item)) {
      item = this.mapProductToCartItem(item);
    }
    currentCart.items = this.addOrUpdateItem(currentCart.items, item, quantity);
    this.setCart(currentCart);
  }

  removeItemFromCart(productId: number, quantity = 1) {
    const cart = this.cart();
    if (!cart) return;

    const index = cart.items.findIndex((x) => x.productId == productId);

    if (index !== -1) {
      if (cart.items[index].quantity > quantity) {
        cart.items[index].quantity -= quantity;
      } else {
        cart.items.splice(index, 1);
      }
      if (cart.items.length === 0) {
        this.deleteCart();
      } else {
        this.setCart(cart);
      }
    }
  }
  deleteCart() {
    this.http.delete(this.baserUrl + 'cart?id=' + this.cart()?.id).subscribe({
      next: () => {
        localStorage.removeItem('cart_id');
        this.cart.set(null);
      },
    });
  }

  private addOrUpdateItem(items: CartItem[], item: CartItem, quantity: number): CartItem[] {
    const index = items.findIndex((x) => x.productId === item.productId);
    if (index === -1) {
      item.quantity = quantity;
      items.push(item);
    } else {
      items[index].quantity += quantity;
    }
    return items;
  }

  private mapProductToCartItem(item: Product): CartItem {
    return {
      productId: item.id,
      productName: item.name,
      price: item.price,
      quantity: 1,
      pictureUrl: item.pictureUrl,
      brand: item.brand,
      type: item.type,
    };
  }

  private CreateCart(): Cart {
    const cart = new Cart();
    localStorage.setItem('cart_id', cart.id);
    this.cart.set(cart);
    return cart;
  }

  private isProduct(item: CartItem | Product): item is Product {
    return (item as Product).id !== undefined;
  }
}
