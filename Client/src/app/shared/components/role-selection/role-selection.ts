import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '../ui/icons/lucide-icons';
import { UserRoles } from '@/app/api/models';

export interface RoleOption {
  value: UserRoles;
  title: string;
  description: string;
  icon: any;
  features: string[];
}

@Component({
  selector: 'app-role-selection',
  templateUrl: './role-selection.html',
  imports: [CommonModule, LucideAngularModule],
})
export class RoleSelection {
  selectedRole = input<UserRoles | null>(UserRoles.User);
  roleChange = output<UserRoles>();
  
  readonly icons = LucideIcons;
  
  roleOptions: RoleOption[] = [
    {
      value: UserRoles.User,
      title: 'Player/Team Manager',
      description: 'Join tournaments and manage your team',
      icon: this.icons.Users,
      features: [
        'Create and manage teams',
        'Register for tournaments', 
        'Add players to your team',
        'Track team performance'
      ]
    },
    {
      value: UserRoles.Organizer,
      title: 'Tournament Organizer',
      description: 'Create and manage tournaments',
      icon: this.icons.Trophy,
      features: [
        'Create tournaments',
        'Manage registrations',
        'Track match results'
      ]
    }
  ];

  onRoleSelect(role: UserRoles) {
    this.roleChange.emit(role);
  }
}