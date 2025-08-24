import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, DashboardLayout],
  template: `
    <app-dashboard-layout>
      <div class="container mx-auto py-8 px-4 md:px-6">
        <header class="mb-8">
          <h1 class="text-2xl font-bold text-card-foreground mb-1">Admin Dashboard</h1>
          <p class="text-muted-foreground">Manage your platform settings and users</p>
        </header>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div class="card p-6">
            <div class="flex flex-col">
              <h3 class="text-sm font-medium text-muted-foreground mb-1">Total Users</h3>
              <p class="text-3xl font-bold">245</p>
              <span class="mt-auto text-xs text-success">+12 this month</span>
            </div>
          </div>
          <div class="card p-6">
            <div class="flex flex-col">
              <h3 class="text-sm font-medium text-muted-foreground mb-1">Active Tournaments</h3>
              <p class="text-3xl font-bold">38</p>
              <span class="mt-auto text-xs text-success">+5 this month</span>
            </div>
          </div>
          <div class="card p-6">
            <div class="flex flex-col">
              <h3 class="text-sm font-medium text-muted-foreground mb-1">Revenue</h3>
              <p class="text-3xl font-bold">$12,450</p>
              <span class="mt-auto text-xs text-success">+8% this month</span>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
          <div class="card p-6">
            <h3 class="font-semibold mb-4">Recent User Registrations</h3>
            <div class="space-y-4">
              <div class="flex items-center justify-between pb-3 border-b border-border">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center">
                    <span class="text-xs font-medium">JD</span>
                  </div>
                  <div>
                    <p class="font-medium">John Doe</p>
                    <p class="text-xs text-muted-foreground">john.doe@example.com</p>
                  </div>
                </div>
                <span class="text-xs text-muted-foreground">2 hours ago</span>
              </div>
              <div class="flex items-center justify-between pb-3 border-b border-border">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center">
                    <span class="text-xs font-medium">JS</span>
                  </div>
                  <div>
                    <p class="font-medium">Jane Smith</p>
                    <p class="text-xs text-muted-foreground">jane.smith@example.com</p>
                  </div>
                </div>
                <span class="text-xs text-muted-foreground">5 hours ago</span>
              </div>
              <div class="flex items-center justify-between pb-3 border-b border-border">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center">
                    <span class="text-xs font-medium">RJ</span>
                  </div>
                  <div>
                    <p class="font-medium">Robert Johnson</p>
                    <p class="text-xs text-muted-foreground">robert.j@example.com</p>
                  </div>
                </div>
                <span class="text-xs text-muted-foreground">Yesterday</span>
              </div>
            </div>
            <div class="mt-4 text-center">
              <a routerLink="/admin/users" class="btn-link text-sm">View all users</a>
            </div>
          </div>

          <div class="card p-6">
            <h3 class="font-semibold mb-4">Platform Activity</h3>
            <div class="space-y-4">
              <div class="flex items-center gap-3 pb-3 border-b border-border">
                <div class="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center">
                  <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-primary">
                    <path d="M18 2H6v6H2v6h4v6h12v-6h4V8h-4V2Z"/>
                    <circle cx="12" cy="8" r="2"/>
                    <circle cx="12" cy="14" r="2"/>
                  </svg>
                </div>
                <div class="flex-1">
                  <p class="font-medium">New tournament created</p>
                  <p class="text-xs text-muted-foreground">Summer League 2025 by Michael Brown</p>
                </div>
                <span class="text-xs text-muted-foreground">1 hour ago</span>
              </div>
              <div class="flex items-center gap-3 pb-3 border-b border-border">
                <div class="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center">
                  <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-primary">
                    <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
                    <circle cx="9" cy="7" r="4"/>
                    <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
                    <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
                  </svg>
                </div>
                <div class="flex-1">
                  <p class="font-medium">New team registered</p>
                  <p class="text-xs text-muted-foreground">FC Barcelona Academy joined Summer League</p>
                </div>
                <span class="text-xs text-muted-foreground">3 hours ago</span>
              </div>
              <div class="flex items-center gap-3 pb-3 border-b border-border">
                <div class="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center">
                  <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="text-primary">
                    <rect width="18" height="18" x="3" y="4" rx="2" ry="2"/>
                    <line x1="16" x2="16" y1="2" y2="6"/>
                    <line x1="8" x2="8" y1="2" y2="6"/>
                    <line x1="3" x2="21" y1="10" y2="10"/>
                    <path d="M8 14h.01"/>
                    <path d="M12 14h.01"/>
                    <path d="M16 14h.01"/>
                    <path d="M8 18h.01"/>
                    <path d="M12 18h.01"/>
                    <path d="M16 18h.01"/>
                  </svg>
                </div>
                <div class="flex-1">
                  <p class="font-medium">Tournament status updated</p>
                  <p class="text-xs text-muted-foreground">Champions Cup changed from Draft to Active</p>
                </div>
                <span class="text-xs text-muted-foreground">Yesterday</span>
              </div>
            </div>
            <div class="mt-4 text-center">
              <a routerLink="/admin/activity" class="btn-link text-sm">View all activity</a>
            </div>
          </div>
        </div>

        <div class="card p-6">
          <h3 class="font-semibold mb-4">Quick Actions</h3>
          <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4">
            <a routerLink="/admin/users" class="btn btn-outline">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2">
                <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/>
                <circle cx="9" cy="7" r="4"/>
                <path d="M22 21v-2a4 4 0 0 0-3-3.87"/>
                <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
              </svg>
              Manage Users
            </a>
            <a routerLink="/admin/tournaments" class="btn btn-outline">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2">
                <path d="M18 2H6v6H2v6h4v6h12v-6h4V8h-4V2Z"/>
                <circle cx="12" cy="8" r="2"/>
                <circle cx="12" cy="14" r="2"/>
              </svg>
              All Tournaments
            </a>
            <a routerLink="/admin/settings" class="btn btn-outline">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2">
                <path d="M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z"/>
                <circle cx="12" cy="12" r="3"/>
              </svg>
              Platform Settings
            </a>
            <a routerLink="/admin/reports" class="btn btn-outline">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2">
                <path d="M21 15V6"/>
                <path d="M18.5 18a2.5 2.5 0 1 0 0-5 2.5 2.5 0 0 0 0 5Z"/>
                <path d="M12 12H3"/>
                <path d="M16 6H3"/>
                <path d="M12 18H3"/>
              </svg>
              Analytics Reports
            </a>
          </div>
        </div>
      </div>
    </app-dashboard-layout>
  `,
})
export class AdminDashboardComponent {}
