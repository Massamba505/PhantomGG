import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  imports: [CommonModule],
  template: `
    <div class="w-full max-w-md border rounded shadow text-center">
      <div class="bg-red-600 text-white py-4 rounded-t">
        <h1 class="text-2xl font-bold">Access Denied</h1>
      </div>

      <div class="p-6 space-y-4">
        <p>You don't have permission to access this page.</p>
        <p>If you believe this is a mistake, contact your administrator.</p>

        <div class="flex justify-center gap-2">
          <button
            pButton
            label="Go Home"
            icon="pi pi-home"
            (click)="navigateTo('/')"
          ></button>
          <button
            pButton
            label="Go Back"
            icon="pi pi-arrow-left"
            class="p-button-secondary"
            (click)="goBack()"
          ></button>
        </div>
      </div>
    </div>
  `,
})
export class UnauthorizedComponent {
  constructor(private router: Router) {}

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }

  goBack(): void {
    window.history.back();
  }
}
