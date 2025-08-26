import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '../icons/lucide-icons';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.html',
  styleUrls: ['./modal.css'],
  imports: [CommonModule, LucideAngularModule],
})
export class Modal {
  @Input() isOpen = false;
  @Input() title = '';
  @Output() close = new EventEmitter<void>();
  readonly icons = LucideIcons;
}
