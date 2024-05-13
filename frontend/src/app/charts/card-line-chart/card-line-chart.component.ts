import {Component, effect, Input, ViewChild, WritableSignal} from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexGrid,
  ApexStroke,
  ApexXAxis,
  ChartComponent,
  NgApexchartsModule
} from 'ng-apexcharts';
import {colors, sharedChartOptions} from "../chart-options";
import {BmeData} from "../../models/bme-data";
import {DatePipe} from "@angular/common";

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  grid: ApexGrid;
  stroke: ApexStroke;
  // title: ApexTitleSubtitle;
};

@Component({
  selector: 'app-card-line-chart',
  standalone: true,
  imports: [
    NgApexchartsModule,
    DatePipe
  ],
  template: `
    <div  id="chart">
      <apx-chart #chart
        [series]="chartOptions.series"
        [chart]="chartOptions.chart"
        [xaxis]="chartOptions.xaxis"
        [dataLabels]="chartOptions.dataLabels"
        [grid]="chartOptions.grid"
        [stroke]="chartOptions.stroke"
        [colors]="sharedChartOptions.colors"
      ></apx-chart>

    </div>`,
  styles: ''
})
export class CardLineChartComponent{

  @ViewChild("chart", {static: false}) chart!: ChartComponent;
  public chartOptions: ChartOptions;
  @Input() mac: string = "";
  @Input() data: WritableSignal<BmeData[]> | undefined;
  bmeData: BmeData[] = [];


  constructor() {
    effect(() => {
      if (this.data) {
        this.bmeData = this.data();
        if (!this.bmeData || !this.chart) return;
        this.chart.updateSeries(
          [
            {
              name: "Temperature",
              data: this.bmeData.map(data => {return {x:new Date(data.createdAt).getTime(), y:data.temperatureC}})
            },
            {
              name: "Humidity",
              data: this.bmeData.map(data => {return {x:new Date(data.createdAt).getTime(), y:data.humidity}})
            },
            // {
            //   name: "Pressure",
            //   data: this.bmeData.map(data => [new Date(data.createdAt).getTime(), data.pressure])
            // }
          ]
        , true);
      }
    });

    this.chartOptions = {

      chart: {
        foreColor: sharedChartOptions.chart.foreColor,
        height: 300,
        type: "line",
        zoom: {
          enabled: false
        }
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        curve: "straight"
      },
      // title: {
      //   text: "Product Trends by Month",
      //   align: "left"
      // },
      grid: {
        row: {
          colors: [colors.base100, "transparent"], // takes an array which will be repeated on columns
          opacity: 0.5
        }
      },
      xaxis: {
        type: "datetime"
      },
      series: [
        {
          name: "Temperature",
          data: this.bmeData.map(data => {return {x:new Date(data.createdAt).getTime(), y:data.temperatureC}})
        },
        {
          name: "Humidity",
          data: this.bmeData.map(data => {return {x:new Date(data.createdAt).getTime(), y:data.humidity}})
        },
        // {
        //   name: "Pressure",
        //   data: this.bmeData.map(data => data.pressure)
        // }
      ],
    };
  }

  protected readonly sharedChartOptions = sharedChartOptions;
}
