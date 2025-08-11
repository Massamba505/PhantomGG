import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { AuthStateService } from '@/app/store/AuthStateService';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, CardModule, ButtonModule, TableModule],
  template: `
    <div class="flex flex-col p-4">
      <h1 class="text-2xl font-bold mb-4">Admin Dashboard</h1>

      <p-card header="User Management" styleClass="mb-4">
        <div class="card-content">
          <p>
            This is a placeholder for the admin dashboard. In a real
            application, you would see user management controls here.
          </p>

          <div class="flex justify-end mt-4">
            <button pButton label="Manage Users" icon="pi pi-users"></button>
          </div>
        </div>
      </p-card>

      <p-card header="System Statistics" styleClass="mb-4">
        <div class="card-content">
          <p>
            This is a placeholder for system statistics. In a real application,
            you would see system statistics here.
          </p>

          <div class="flex justify-end mt-4">
            <button
              pButton
              label="View Reports"
              icon="pi pi-chart-bar"
            ></button>
          </div>
        </div>
      </p-card>
    </div>
  `,
})
export class AdminComponent implements OnInit {
  authState = inject(AuthStateService);

  ngOnInit() {
    console.log('Admin component initialized');

    // In a real application, you would load admin data here
    console.log('Current user role:', this.authState.user()?.role);
  }
}
