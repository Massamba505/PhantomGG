import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '../ui/icons/lucide-icons';
import { Roles } from '../../constants/roles';

export interface RoleOption {
  value: Roles;
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
  selectedRole = input<Roles | null>(Roles.User);
  roleChange = output<Roles>();
  
  readonly icons = LucideIcons;
  
  roleOptions: RoleOption[] = [
    {
      value: Roles.User,
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
      value: Roles.Organizer,
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

  onRoleSelect(role: Roles) {
    this.roleChange.emit(role);
  }
}