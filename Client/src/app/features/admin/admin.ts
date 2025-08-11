import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin',
  imports: [CommonModule],
  template: `
    <div class="flex flex-col p-4">
      <h1 class="text-2xl font-bold mb-4">Admin Dashboard</h1>

      <div header="User Management" class="card mb-4">
        <div class="card-content">
          <p>
            This is a placeholder for the admin dashboard. In a real
            application, you would see user management controls here.
          </p>

          <div class="flex justify-end mt-4">
            <button pButton label="Manage Users" icon="pi pi-users"></button>
          </div>
        </div>
      </div>

      <div header="System Statistics" class="card mb-4">
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
      </div>
    </div>
  `,
})
export class Admin {}
