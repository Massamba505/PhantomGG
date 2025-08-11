import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [CommonModule, ButtonModule, CardModule],
  template: `
    <div class="flex justify-center items-center h-screen">
      <p-card styleClass="w-full max-w-md">
        <ng-template pTemplate="header">
          <div class="bg-red-600 text-white text-center py-4">
            <h1 class="text-2xl font-bold">Access Denied</h1>
          </div>
        </ng-template>

        <div class="p-4 text-center">
          <p class="mb-4">You don't have permission to access this page.</p>
          <p class="mb-4">
            Please contact an administrator if you believe this is an error.
          </p>

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
              (click)="goBack()"
              class="p-button-secondary"
            ></button>
          </div>
        </div>
      </p-card>
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
