import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastService } from '@/app/shared/services/toast.service';
import { Modal } from '@/app/shared/components/ui/modal/modal';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, RouterLink, DashboardLayout, ReactiveFormsModule, FormsModule, Modal],
  template: `
    <app-dashboard-layout>
      <div class="container mx-auto py-8 px-4 md:px-6">
        <header class="mb-6">
          <div class="flex items-center gap-2 mb-1">
            <a routerLink="/admin" class="transition-colors flex items-center">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-1">
                <path d="m15 18-6-6 6-6"/>
              </svg>
              <span>Back to Admin Dashboard</span>
            </a>
          </div>
          <h1 class="text-2xl font-bold mb-1">Manage Users</h1>
          <p class="">View and manage all users on your platform</p>
        </header>

        <!-- User Management Controls -->
        <div class="card p-6 mb-6">
          <div class="flex flex-col md:flex-row gap-4 justify-between items-start md:items-center">
            <div class="relative flex-1 max-w-md">
              <div class="absolute inset-y-0 left-0 flex items-center pl-3 pointer-events-none ">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <circle cx="11" cy="11" r="8"/>
                  <path d="m21 21-4.3-4.3"/>
                </svg>
              </div>
              <input
                type="text"
                placeholder="Search users..."
                [(ngModel)]="searchQuery"
                (ngModelChange)="filterUsers()"
                class="form-input pl-10 w-full"
              />
            </div>

            <div class="flex gap-3">
              <select [(ngModel)]="roleFilter" (ngModelChange)="filterUsers()" class="form-select">
                <option value="all">All Roles</option>
                <option value="admin">Admin</option>
                <option value="user">User</option>
                <option value="organizer">Organizer</option>
              </select>
              
              <button (click)="showAddUserModal = true" class="btn btn-primary">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="mr-2">
                  <path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
                  <circle cx="8.5" cy="7" r="4"/>
                  <line x1="20" x2="20" y1="8" y2="14"/>
                  <line x1="23" x2="17" y1="11" y2="11"/>
                </svg>
                Add User
              </button>
            </div>
          </div>
        </div>

        <!-- Users Table -->
        <div class="card">
          <div class="overflow-x-auto">
            <table class="w-full">
              <thead>
                <tr class="border-b border-border">
                  <th class="text-left py-3 px-4 font-medium ">Name</th>
                  <th class="text-left py-3 px-4 font-medium ">Email</th>
                  <th class="text-left py-3 px-4 font-medium ">Role</th>
                  <th class="text-left py-3 px-4 font-medium ">Status</th>
                  <th class="text-left py-3 px-4 font-medium ">Joined</th>
                  <th class="text-left py-3 px-4 font-medium ">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let user of filteredUsers" class="border-b border-border transition-colors">
                  <td class="py-3 px-4">
                    <div class="flex items-center gap-3">
                      <div class="w-8 h-8 rounded-full bg-primary/20 flex items-center justify-center">
                        <span class="text-xs font-medium">{{ user.firstName.charAt(0) }}{{ user.lastName.charAt(0) }}</span>
                      </div>
                      <span class="font-medium">{{ user.firstName }} {{ user.lastName }}</span>
                    </div>
                  </td>
                  <td class="py-3 px-4">{{ user.email }}</td>
                  <td class="py-3 px-4">
                    <span class="badge" [ngClass]="
                      user.role === 'admin' ? 'badge-primary' : 
                      user.role === 'organizer' ? 'badge-success' : 'badge-muted'
                    ">
                      {{ user.role }}
                    </span>
                  </td>
                  <td class="py-3 px-4">
                    <span class="badge" [ngClass]="user.isActive ? 'badge-success' : 'badge-muted'">
                      {{ user.isActive ? 'Active' : 'Inactive' }}
                    </span>
                  </td>
                  <td class="py-3 px-4 ">{{ user.joinedDate }}</td>
                  <td class="py-3 px-4">
                    <div class="flex items-center gap-2">
                      <button class="btn-icon btn-ghost" (click)="editUser(user)">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                          <path d="M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5Z"/>
                          <path d="m15 5 4 4"/>
                        </svg>
                      </button>
                      <button class="btn-icon btn-ghost text-destructive" (click)="confirmDeleteUser(user)">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                          <path d="M3 6h18"/>
                          <path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/>
                          <path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/>
                          <line x1="10" x2="10" y1="11" y2="17"/>
                          <line x1="14" x2="14" y1="11" y2="17"/>
                        </svg>
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          
          <div *ngIf="filteredUsers.length === 0" class="py-12 text-center">
            <div class="w-16 h-16 mx-auto mb-4  rounded-full flex items-center justify-center">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="">
                <path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
                <circle cx="8.5" cy="7" r="4"/>
                <path d="M20 8v6"/>
                <path d="M23 11h-6"/>
              </svg>
            </div>
            <h3 class="text-lg font-semibold mb-2">No users found</h3>
            <p class=" mb-6">Try adjusting your search or filters</p>
          </div>
          
          <!-- Pagination -->
          <div class="p-4 border-t border-border flex justify-between items-center">
            <div class="text-sm ">
              Showing <span class="font-medium">1</span> to <span class="font-medium">{{ filteredUsers.length }}</span> of <span class="font-medium">{{ users.length }}</span> users
            </div>
            <div class="flex gap-2">
              <button class="btn-icon btn-ghost" [disabled]="currentPage === 1">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="m15 18-6-6 6-6"/>
                </svg>
              </button>
              <button class="btn-icon btn-ghost" [disabled]="currentPage === totalPages">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="m9 18 6-6-6-6"/>
                </svg>
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Add User Modal -->
      <app-modal
        [isOpen]="showAddUserModal"
        (close)="showAddUserModal = false"
        title="Add New User"
      >
        <form [formGroup]="userForm" (ngSubmit)="addUser()" class="space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium mb-2">First Name</label>
              <input type="text" formControlName="firstName" class="form-input" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-2">Last Name</label>
              <input type="text" formControlName="lastName" class="form-input" />
            </div>
          </div>
          
          <div>
            <label class="block text-sm font-medium mb-2">Email Address</label>
            <input type="email" formControlName="email" class="form-input" />
          </div>
          
          <div>
            <label class="block text-sm font-medium mb-2">Role</label>
            <select formControlName="role" class="form-select">
              <option value="user">User</option>
              <option value="organizer">Organizer</option>
              <option value="admin">Admin</option>
            </select>
          </div>
          
          <div class="flex items-center gap-2">
            <input type="checkbox" id="isActive" formControlName="isActive" class="form-checkbox" />
            <label for="isActive" class="text-sm font-medium">Active Account</label>
          </div>
          
          <div class="flex justify-end gap-3 mt-6">
            <button type="button" class="btn btn-outline" (click)="showAddUserModal = false">Cancel</button>
            <button type="submit" class="btn btn-primary">Add User</button>
          </div>
        </form>
      </app-modal>
      
      <!-- Edit User Modal -->
      <app-modal
        [isOpen]="showEditUserModal"
        (close)="showEditUserModal = false"
        title="Edit User"
      >
        <form [formGroup]="userForm" (ngSubmit)="updateUser()" class="space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium mb-2">First Name</label>
              <input type="text" formControlName="firstName" class="form-input" />
            </div>
            <div>
              <label class="block text-sm font-medium mb-2">Last Name</label>
              <input type="text" formControlName="lastName" class="form-input" />
            </div>
          </div>
          
          <div>
            <label class="block text-sm font-medium mb-2">Email Address</label>
            <input type="email" formControlName="email" class="form-input" />
          </div>
          
          <div>
            <label class="block text-sm font-medium mb-2">Role</label>
            <select formControlName="role" class="form-select">
              <option value="user">User</option>
              <option value="organizer">Organizer</option>
              <option value="admin">Admin</option>
            </select>
          </div>
          
          <div class="flex items-center gap-2">
            <input type="checkbox" id="isActiveEdit" formControlName="isActive" class="form-checkbox" />
            <label for="isActiveEdit" class="text-sm font-medium">Active Account</label>
          </div>
          
          <div class="flex justify-end gap-3 mt-6">
            <button type="button" class="btn btn-outline" (click)="showEditUserModal = false">Cancel</button>
            <button type="submit" class="btn btn-primary">Update User</button>
          </div>
        </form>
      </app-modal>
    </app-dashboard-layout>
  `,
  styles: [`
    .form-input,
    .form-select {
      width: 100%;
      padding: 0.5rem 0.75rem;
      border-radius: 0.5rem;
      border: 1px solid rgb(var(--border));
      background-color: rgb(var(--background));
      color: rgb(var(--foreground));
    }
    
    .form-input:focus,
    .form-select:focus {
      outline: none;
      border-color: rgb(var(--primary));
      box-shadow: 0 0 0 1px rgba(var(--primary), 0.5);
    }
    
    .form-checkbox {
      border-radius: 0.25rem;
      border: 1px solid rgb(var(--border));
      background-color: rgb(var(--background));
    }
    
    .badge {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      border-radius: 9999px;
      padding: 0.125rem 0.5rem;
      font-size: 0.75rem;
      font-weight: 500;
    }
    
    .badge-primary {
      background-color: rgba(var(--primary), 0.2);
      color: rgb(var(--primary));
    }
    
    .badge-success {
      background-color: rgba(var(--success), 0.2);
      color: rgb(var(--success));
    }
    
    .badge-muted {
      background-color: rgb(var(--muted));
      color: rgb(var(--muted-foreground));
    }
    
    .btn-icon {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      width: 2rem;
      height: 2rem;
      border-radius: 0.5rem;
      transition: background-color 0.2s ease, color 0.2s ease;
    }
    
    .btn-ghost {
      background-color: transparent;
    }
    
    .btn-ghost:hover {
      background-color: rgba(var(--muted), 0.5);
    }
  `]
})
export class AdminUsersComponent {
  searchQuery = '';
  roleFilter = 'all';
  currentPage = 1;
  totalPages = 1;
  showAddUserModal = false;
  showEditUserModal = false;
  editingUserId: string | null = null;
  
  userForm: FormGroup;
  
  users = [
    {
      id: '1',
      firstName: 'John',
      lastName: 'Doe',
      email: 'john.doe@example.com',
      role: 'admin',
      isActive: true,
      joinedDate: 'Aug 10, 2025'
    },
    {
      id: '2',
      firstName: 'Jane',
      lastName: 'Smith',
      email: 'jane.smith@example.com',
      role: 'organizer',
      isActive: true,
      joinedDate: 'Aug 15, 2025'
    },
    {
      id: '3',
      firstName: 'Robert',
      lastName: 'Johnson',
      email: 'robert.j@example.com',
      role: 'user',
      isActive: true,
      joinedDate: 'Aug 20, 2025'
    },
    {
      id: '4',
      firstName: 'Emily',
      lastName: 'Williams',
      email: 'emily.w@example.com',
      role: 'user',
      isActive: false,
      joinedDate: 'Aug 5, 2025'
    },
    {
      id: '5',
      firstName: 'Michael',
      lastName: 'Brown',
      email: 'michael.b@example.com',
      role: 'organizer',
      isActive: true,
      joinedDate: 'July 28, 2025'
    }
  ];
  
  filteredUsers: any[] = [];
  
  constructor(
    private fb: FormBuilder,
    private toast: ToastService
  ) {
    this.userForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      role: ['user', Validators.required],
      isActive: [true]
    });
    
    this.filterUsers();
  }
  
  filterUsers() {
    this.filteredUsers = this.users.filter(user => {
      const matchesSearch = 
        this.searchQuery === '' ||
        user.firstName.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        user.lastName.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        user.email.toLowerCase().includes(this.searchQuery.toLowerCase());
        
      const matchesRole = 
        this.roleFilter === 'all' || 
        user.role === this.roleFilter;
        
      return matchesSearch && matchesRole;
    });
  }
  
  editUser(user: any) {
    this.editingUserId = user.id;
    this.userForm.patchValue({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      role: user.role,
      isActive: user.isActive
    });
    this.showEditUserModal = true;
  }
  
  addUser() {
    if (this.userForm.invalid) return;
    
    const newUser = {
      id: (this.users.length + 1).toString(),
      ...this.userForm.value,
      joinedDate: 'Aug 24, 2025'
    };
    
    this.users.unshift(newUser);
    this.filterUsers();
    this.showAddUserModal = false;
    this.userForm.reset({
      firstName: '',
      lastName: '',
      email: '',
      role: 'user',
      isActive: true
    });
    
    this.toast.success('User added successfully');
  }
  
  updateUser() {
    if (this.userForm.invalid || !this.editingUserId) return;
    
    const userIndex = this.users.findIndex(u => u.id === this.editingUserId);
    if (userIndex !== -1) {
      this.users[userIndex] = {
        ...this.users[userIndex],
        ...this.userForm.value
      };
      
      this.filterUsers();
      this.showEditUserModal = false;
      this.editingUserId = null;
      
      this.toast.success('User updated successfully');
    }
  }
  
  confirmDeleteUser(user: any) {
    if (confirm(`Are you sure you want to delete ${user.firstName} ${user.lastName}?`)) {
      this.users = this.users.filter(u => u.id !== user.id);
      this.filterUsers();
      this.toast.success('User deleted successfully');
    }
  }
}
