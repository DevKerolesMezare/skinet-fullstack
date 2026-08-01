import { HttpClient } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-test-error',
  imports: [MatButton],
  templateUrl: './test-error.component.html',
  styleUrl: './test-error.component.scss',
})
export class TestErrorComponent {
  // env. base
  baseUrl = 'https://localhost:5001/api/';
  private http = inject(HttpClient);
  validationErors = signal<string[] | null>(null);

  get404Error() {
    this.http.get(this.baseUrl + 'buggy/notfound').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get400Error() {
    this.http.get(this.baseUrl + 'buggy/badrequest').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get500Error() {
    this.http.get(this.baseUrl + 'buggy/internalerror').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get401Error() {
    this.http.get(this.baseUrl + 'buggy/unauthorized').subscribe({
      next: (response) => console.log(response),
      error: (error) => console.log(error),
    });
  }

  get400ValidationError() {
    this.http
      .post(this.baseUrl + 'buggy/validationerror', {
        name: '',
        description: '',
        pictureUrl: '',
        price: 0,
        type: '',
        brand: '',
      })
      .subscribe({
        next: (response) => console.log(response),
        error: (error) => this.validationErors.set(error),
      });
  }
}
