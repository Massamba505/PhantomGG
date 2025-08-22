import { Component, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Roles } from '@/app/shared/constants/roles';
import { Sidebar } from '../../sidebar/sidebar';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [CommonModule, Sidebar],
  templateUrl: './dashboard-layout.html',
})
export class DashboardLayout implements OnInit, OnDestroy {
  sidebarOpen = signal(window.innerWidth > 768);

  userRole: Roles = Roles.Organizer;

  ngOnInit() {
    window.addEventListener('resize', this.handleResize);
  }

  ngOnDestroy() {
    window.removeEventListener('resize', this.handleResize);
  }

  handleResize = () => {
    this.sidebarOpen.set(window.innerWidth > 768);
  };

  toggleSidebar() {
    this.sidebarOpen.update((prev) => !prev);
  }
}
