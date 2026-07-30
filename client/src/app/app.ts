import { Component, signal } from '@angular/core';
import { HeaderComponent } from './layout/header/header.component';

import { ShopComponent } from "./features/shop/shop.component";

@Component({
  selector: 'app-root',
  imports: [ HeaderComponent, ShopComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('Skinet');

}
