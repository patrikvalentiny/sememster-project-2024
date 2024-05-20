import {Component, inject, Input, WritableSignal} from '@angular/core';
import {Device} from "../../models/device";
import {DatePipe, NgClass} from "@angular/common";
import {BmeData} from "../../models/bme-data";
import {Router} from "@angular/router";
import {CardLineChartComponent} from "../../charts/card-line-chart/card-line-chart.component";

@Component({
  selector: 'app-device-card',
  standalone: true,
  imports: [
    NgClass,
    DatePipe,
    CardLineChartComponent
  ],
  templateUrl: './device-card.component.html',
  styleUrl: './device-card.component.css'
})
export class DeviceCardComponent {
  @Input() device: Device = {id: 0, name: "distinguished-yak", mac: "69420", status_id: 0, status: "offline"};
  @Input() bmeData: WritableSignal<BmeData[]> | undefined;
  private readonly router = inject(Router);

  async motorControlNavigation() {
    await this.router.navigate(["/motor", this.device.mac]);
  }

  async dataNavigation() {
    await this.router.navigate(["/data", this.device.mac]);
  }
}
