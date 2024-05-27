import {Component, inject, Input} from '@angular/core';
import {Device} from "../../models/device";
import {Router} from "@angular/router";

@Component({
  selector: 'app-device-sidebar-item',
  standalone: true,
  imports: [],
  templateUrl: './device-sidebar-item.component.html',
  styleUrl: './device-sidebar-item.component.css'
})
export class DeviceSidebarItemComponent {
  @Input() device!: Device;
  private readonly router = inject(Router);

  async viewData() {
    await this.router.navigate(["/data", this.device.mac]);
  }

  async controlMotor() {
    await this.router.navigate(["/motor", this.device.mac]);
  }
}
