import {Component, effect, Input, signal, ViewChild, WritableSignal} from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexGrid,
  ApexStroke, ApexXAxis,
  ChartComponent,
  NgApexchartsModule
} from "ng-apexcharts";
import {colors, sharedChartOptions} from "../chart-options";
import {BmeData} from "../../models/bme-data";
import {NgClass} from "@angular/common";

type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  grid: ApexGrid;
  stroke: ApexStroke;
};

@Component({
  selector: 'app-historic-data-chart',
  standalone: true,
  imports: [
    NgApexchartsModule,
    NgClass
  ],
  template: `

    <div id="chart">
      <apx-chart #chart
                 [series]="chartOptions.series"
                 [chart]="chartOptions.chart"
                 [xaxis]="chartOptions.xaxis"
                 [dataLabels]="chartOptions.dataLabels"
                 [grid]="chartOptions.grid"
                 [stroke]="chartOptions.stroke"
                 [colors]="sharedChartOptions.colors"
                 [theme]="sharedChartOptions.theme"
      ></apx-chart>

    </div>`,
  styles: ''
})
export class HistoricTemperatureDataChartComponent {
  @Input() bmeData: WritableSignal<BmeData[]> = signal([]);
  @ViewChild("chart", {static: false}) chart!: ChartComponent;
  public chartOptions: ChartOptions;
  protected readonly sharedChartOptions = sharedChartOptions;

  constructor() {
    effect(() => {
      this.updateSeries(this.bmeData()).then();
    });
    this.chartOptions = {
      chart: {
        foreColor: sharedChartOptions.chart.foreColor,
        background: sharedChartOptions.chart.background,
        height: 300,
        type: "line",
        zoom: {
          enabled: true,
          type: "x",
          autoScaleYaxis: true
        }
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        curve: "smooth"
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
          data: this.bmeData().map(data => {
            return {x: new Date(data.createdAt).getTime(), y: data.temperatureC}
          })
        },
        // {
        //   name: "Humidity",
        //   data: this.bmeData.map(data => {
        //     return {x: new Date(data.createdAt).getTime(), y: data.humidity}
        //   })
        // },
        // {
        //   name: "Pressure",
        //   data: this.bmeData.map(data => data.pressure)
        // }
      ],
    };
  }


  async updateSeries(data: BmeData[]) {
    this.chart.updateSeries(
      [
        {
          name: "Temperature",
          data: data.map(data => {
            return {x: new Date(data.createdAt).getTime(), y: data.temperatureC}
          })
        },
        // {
        //   name: "Humidity",
        //   data: this.bmeData.map(data => {
        //     return {x: new Date(data.createdAt).getTime(), y: data.humidity}
        //   })
        // },
        // {
        //   name: "Pressure",
        //   data: this.bmeData.map(data => data.pressure)
        // }
      ]
    );
  }

}
