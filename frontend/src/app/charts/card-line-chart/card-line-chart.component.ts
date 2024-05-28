import {Component, effect, Input, ViewChild, WritableSignal} from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexGrid,
  ApexStroke,
  ApexTitleSubtitle,
  ApexTooltip,
  ApexXAxis,
  ChartComponent,
  NgApexchartsModule
} from 'ng-apexcharts';
import {colors, sharedChartOptions} from "../chart-options";
import {BmeData} from "../../models/bme-data";
import {DatePipe} from "@angular/common";

type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  grid: ApexGrid;
  stroke: ApexStroke;
  title: ApexTitleSubtitle;
  tooltip: ApexTooltip;
};

@Component({
  selector: 'app-card-line-chart',
  standalone: true,
  imports: [
    NgApexchartsModule,
    DatePipe
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
                 [title]="chartOptions.title"
                 [tooltip]="chartOptions.tooltip"
      ></apx-chart>

    </div>`,
  styles: ''
})
export class CardLineChartComponent {

  @ViewChild("chart", {static: false}) chart!: ChartComponent;
  public chartOptions: ChartOptions;
  @Input() data: WritableSignal<BmeData[]> | undefined;
  bmeData: BmeData[] = [];
  protected readonly sharedChartOptions = sharedChartOptions;

  constructor() {
    effect(() => {
      if (this.data) {
        this.bmeData = this.data();
        if (!this.bmeData || !this.chart) return;
        this.chart.updateSeries(
          [
            {
              name: "Temperature",
              data: this.formatData()
            },
          ]
        );
      }
    });

    this.chartOptions = {
      tooltip: {
        shared: true,
        onDatasetHover: {
          highlightDataSeries: true
        },
        x: {
          format: "dd/MM/yy HH:mm"
        }
      },
      chart: {
        animations: {
          enabled: true,
          easing: 'easeout',
          dynamicAnimation: {
            speed: 250
          }
        },
        foreColor: sharedChartOptions.chart.foreColor,
        background: sharedChartOptions.chart.background,
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
        curve: "smooth"
      },
      title: {
        text: "Temperature last 24 hours",
        align: "left"
      },
      grid: {
        row: {
          colors: [colors.base100, "transparent"], // takes an array which will be repeated on columns
          opacity: 0.5
        }
      },
      xaxis: {
        type: "datetime",
        range: 24 * 60 * 60 * 1000,
        labels: {
          format: "HH:mm",
          datetimeUTC: false
        }
      },
      series: [
        {
          name: "Temperature",
          data: this.formatData()
        },
      ],
    };
  }

  formatData() {
    const decimalPlaces: number = 3;
    const decimalMultiplier = Math.pow(10, decimalPlaces);
    return this.bmeData
    //   .filter(
    //   data => new Date(data.createdAt).getMinutes() % 10 === 0
    // )
      .map(data => {
      return {
        x: new Date(data.createdAt).getTime(),
        y: Math.round(data.temperatureC * decimalMultiplier) / decimalMultiplier
      }
    })


  }
}
