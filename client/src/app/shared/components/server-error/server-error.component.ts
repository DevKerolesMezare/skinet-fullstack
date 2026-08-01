import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatCard } from "@angular/material/card";

@Component({
  selector: 'app-server-error',
  imports: [MatCard],
  templateUrl: './server-error.component.html',
  styleUrl: './server-error.component.scss',
})
export class ServerErrorComponent {
  error = signal<any>(null);
  private router = inject(Router);

  constructor() {
    const navigation = this.router.currentNavigation();

    this.error.set(navigation?.extras.state?.['error']);
  }
}
