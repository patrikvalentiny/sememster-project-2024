import {Component, effect, inject, input, InputSignal, signal, WritableSignal} from '@angular/core';
import {HistoricTemperatureDataChartComponent} from "../../charts/historic-data-chart/historic-temperature-data-chart.component";
import {BmeData} from "../../models/bme-data";
import {DataService} from "../../services/data.service";
import {NgClass} from "@angular/common";


@Component({
  selector: 'app-historic-data',
  standalone: true,
  imports: [
    HistoricTemperatureDataChartComponent,
    NgClass
  ],
  templateUrl: './historic-data.component.html',
  styleUrl: './historic-data.component.css'
})
export class HistoricDataComponent{
  private readonly dataService = inject(DataService);
  mac:InputSignal<string> = input("", {alias:"mac"});
  bmeData:WritableSignal<BmeData[]> = signal<BmeData[]>([]);
  days:number = 7;


  constructor() {
    effect(() => {
      this.getBmeData(this.days).then()
    });
  }

  async getBmeData(number: number) {
    this.days = number;
    this.bmeData.set(await this.dataService.getDataPastXDays(this.mac(), number));
  }
}
