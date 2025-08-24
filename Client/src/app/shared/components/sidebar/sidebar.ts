import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

import {
  LucideAngularModule,
  Home,
  Trophy,
  Users,
  Calendar,
  PlusCircle,
  Bell,
  Menu,
} from 'lucide-angular';
import { Roles } from '../../constants/roles';
import { Footer } from '../footer/footer';

interface MenuItem {
  title: string;
  url: string;
  icon: any;
  roles: Roles[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, LucideAngularModule, Footer],
  templateUrl: './sidebar.html',
})
export class Sidebar implements OnInit, OnDestroy {
  @Input() isOpen = true;
  @Input() userRole!: Roles;
  @Output() toggle = new EventEmitter<void>();

  MenuIcon = Menu;
  menuItems: MenuItem[] = [
    {
      title: 'Dashboard',
      url: '/dashboard',
      icon: Home,
      roles: [Roles.Organizer, Roles.General],
    },
    {
      title: 'Tournaments',
      url: '/tournaments',
      icon: Trophy,
      roles: [Roles.Organizer],
    },
    { title: 'Teams', url: '/teams', icon: Users, roles: [Roles.General] },
    {
      title: 'Fixtures',
      url: '/fixtures',
      icon: Calendar,
      roles: [Roles.Organizer, Roles.General],
    },
    {
      title: 'Notifications',
      url: '/notifications',
      icon: Bell,
      roles: [Roles.Organizer, Roles.General],
    },
  ];

  quickActions: MenuItem[] = [
    {
      title: 'Create Tournament',
      url: '/create-tournament',
      icon: PlusCircle,
      roles: [Roles.Organizer],
    },
    {
      title: 'Create Team',
      url: '/create-team',
      icon: PlusCircle,
      roles: [Roles.General],
    },
  ];

  ngOnInit() {
    window.addEventListener('resize', this.handleResize);
  }

  ngOnDestroy() {
    window.removeEventListener('resize', this.handleResize);
  }

  handleResize = () => {
    this.isOpen = window.innerWidth > 768;
  };
}
