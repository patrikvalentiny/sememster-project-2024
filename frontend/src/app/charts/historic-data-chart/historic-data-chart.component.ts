import {Component, effect, Input, signal, ViewChild, WritableSignal} from '@angular/core';
import {
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexGrid,
  ApexStroke, ApexTooltip,
  ApexXAxis,
  ChartComponent,
  NgApexchartsModule
} from "ng-apexcharts";
import {colors, sharedChartOptions} from "../chart-options";
import {BmeData} from "../../models/bme-data";
import {NgClass} from "@angular/common";

type ChartOptions = {
  series: ApexAxisChartSeries;
  // chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  grid: ApexGrid;
  stroke: ApexStroke;
  tooltip:ApexTooltip;
};

@Component({
  selector: 'app-historic-data-chart',
  standalone: true,
  imports: [
    NgApexchartsModule,
    NgClass
  ],
  template: `

    <div class="flex flex-col gap-2 w-full overflow-hidden" id="chart">
      <apx-chart #chart1
                 [series]="commonChartOptions.series"
                 [chart]="chart1Options"
                 [xaxis]="commonChartOptions.xaxis"
                 [dataLabels]="commonChartOptions.dataLabels"
                 [grid]="commonChartOptions.grid"
                 [stroke]="commonChartOptions.stroke"
                 [colors]="sharedChartOptions.colors"
                 [theme]="sharedChartOptions.theme"
                 [title]="{text: 'Temperature'}"
                 [tooltip]="commonChartOptions.tooltip"
      ></apx-chart>
      <apx-chart #chart2
                 [series]="commonChartOptions.series"
                 [chart]="chart2Options"
                 [xaxis]="commonChartOptions.xaxis"
                 [dataLabels]="commonChartOptions.dataLabels"
                 [grid]="commonChartOptions.grid"
                 [stroke]="commonChartOptions.stroke"
                 [colors]="[sharedChartOptions.colors.at(1)]"
                 [theme]="sharedChartOptions.theme"
                 [title]="{text: 'Humidity'}"
                 [tooltip]="commonChartOptions.tooltip"
      ></apx-chart>
      <apx-chart #chart3
                 [series]="commonChartOptions.series"
                 [chart]="chart3Options"
                 [xaxis]="commonChartOptions.xaxis"
                 [dataLabels]="commonChartOptions.dataLabels"
                 [grid]="commonChartOptions.grid"
                 [stroke]="commonChartOptions.stroke"
                 [colors]="[sharedChartOptions.colors.at(2)]"
                 [theme]="sharedChartOptions.theme"
                 [title]="{text: 'Pressure'}"
                 [tooltip]="commonChartOptions.tooltip"
      ></apx-chart>

    </div>`,
  styles: ''
})
export class HistoricDataChartComponent {
  @Input() bmeData: WritableSignal<BmeData[]> = signal([]);
  @ViewChild("chart1", {static: false}) chart1!: ChartComponent;
  @ViewChild("chart2", {static: false}) chart2!: ChartComponent;
  @ViewChild("chart3", {static: false}) chart3!: ChartComponent;

  public commonChartOptions: ChartOptions;
  public chart1Options: ApexChart;
  public chart2Options: ApexChart;
  public chart3Options: ApexChart;
  protected readonly sharedChartOptions = sharedChartOptions;

  constructor() {
    effect(() => {
      if (this.bmeData) {
        this.updateSeries(this.bmeData()).then();
      }
    });

    this.commonChartOptions = {
      tooltip:{
        shared:true,
        onDatasetHover:{
          highlightDataSeries: true
        },
        x:{
          format:"dd/MM/yy HH:mm"
        }
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        curve: "smooth"
      },
      grid: {
        row: {
          colors: [colors.base100, "transparent"], // takes an array which will be repeated on columns
          opacity: 0.5
        }
      },
      xaxis: {
        type: "datetime",
        labels: {
          format: "dd/MM/yy HH:mm",
          datetimeUTC: false
        }
      },
      series: []
    };

    this.chart1Options = {
      animations: {
        enabled: true,
        easing: 'easeout',
        dynamicAnimation: {
          speed: 1000
        }
      },
      id: "chart1",
      group:"bmedata",
      foreColor: sharedChartOptions.chart.foreColor,
      background: sharedChartOptions.chart.background,
      height: 300,
      type: "line",
      zoom: {
        enabled: true,
        type: "x",
        autoScaleYaxis: true
      }
    };
    this.chart2Options = {
      animations: {
        enabled: true,
        easing: 'easeout',
        dynamicAnimation: {
          speed: 1000
        }
      },
      id: "chart2",
      group:"bmedata",
      foreColor: sharedChartOptions.chart.foreColor,
      background: sharedChartOptions.chart.background,
      height: 300,
      type: "line",
      zoom: {
        enabled: true,
        type: "x",
        autoScaleYaxis: true
      }

    };
    this.chart3Options = {
      animations: {
        enabled: true,
        easing: 'easeout',
        dynamicAnimation: {
          speed: 1000
        }
      },
      id: "chart3",
      group:"bmedata",
      foreColor: sharedChartOptions.chart.foreColor,
      background: sharedChartOptions.chart.background,
      height: 300,
      type: "line",
      zoom: {
        enabled: true,
        type: "x",
        autoScaleYaxis: true
      }
    }
  }

  async updateSeries(data: BmeData[]) {
    this.chart1.updateSeries(
      [
        {
          name: "Temperature",
          data: data.map(data => {
            return {x: new Date(data.createdAt).getTime(), y: data.temperatureC}
          })
        },
      ]
    );
    this.chart2.updateSeries(
      [
        {
          name: "Humidity",
          data: data.map(data => {
            return {x: new Date(data.createdAt).getTime(), y: data.humidity}
          })
        },
      ]);
    this.chart3.updateSeries(
      [
        {
          name: "Pressure",
          data: data.map(data => {
            return {x: new Date(data.createdAt).getTime(), y: data.pressure}
          })
        },
      ]);
  }

}
