import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Modal } from "../modal/modal";
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '../ui/icons/lucide-icons';

@Component({
  selector: 'app-confirm-delete-modal',
  imports: [Modal, LucideAngularModule],
  templateUrl: "./ConfirmDeleteModal.html",
  styleUrl: './ConfirmDeleteModal.css',
})
export class ConfirmDeleteModal { 
  @Input() isOpen: boolean = false;
  @Input() title: string = 'Delete';
  @Input() entityName: string = '';
  @Output() close = new EventEmitter<void>();
  @Output() confirm = new EventEmitter<void>();

  readonly icons = LucideIcons;

  handleClose() {
    this.close.emit();
  }

  handleConfirm() {
    this.confirm.emit();
  }

}
