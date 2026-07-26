import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './layout/header/header.component';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {
  protected readonly title = signal('Skinet');


  // sample test
  baseUrl = 'https://localhost:5001/api/';
  private http = inject(HttpClient);

  constructor() {
    this.http.get(this.baseUrl + 'Products').subscribe({
      next: (res) => console.log(res),
    });
  }
}
