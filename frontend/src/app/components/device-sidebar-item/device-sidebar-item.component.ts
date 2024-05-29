import {Component, inject, Input, OnInit} from '@angular/core';
import {Device} from "../../models/device";
import {Router} from "@angular/router";
import {NgClass} from "@angular/common";

@Component({
  selector: 'app-device-sidebar-item',
  standalone: true,
  imports: [
    NgClass
  ],
  templateUrl: './device-sidebar-item.component.html',
  styleUrl: './device-sidebar-item.component.css'
})
export class DeviceSidebarItemComponent {
  @Input() device!: Device;
  readonly router = inject(Router);

  async viewData() {
    await this.router.navigate(["/data", this.device.mac]);
  }

  async controlMotor() {
    await this.router.navigate(["/motor", this.device.mac]);
  }
}
