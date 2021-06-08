
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {  Component,ChangeDetectionStrategy, Injector} from '@angular/core';

import { Service, ComplaintsWithPercent } from '../home.service';

@Component({
  templateUrl: './home.component.html',
  animations: [appModuleAnimation()],
  providers:[Service],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent extends AppComponentBase {
  dataSource: ComplaintsWithPercent[];

    constructor(service: Service,injector: Injector) {
      super(injector)
        this.dataSource = service.getComplaintsData()
    }

    customizeTooltip = (info: any) => {
        return {
            html: "<div><div class='tooltip-header'>" +
                info.argumentText + "</div>" +
                "<div class='tooltip-body'><div class='series-name'>" +
                "<span class='top-series-name'>" + info.points[0].seriesName + "</span>" +
                ": </div><div class='value-text'>" +
                "<span class='top-series-value'>" + info.points[0].valueText + "</span>" +
                "</div><div class='series-name'>" +
                "<span class='bottom-series-name'>" + info.points[1].seriesName + "</span>" +
                ": </div><div class='value-text'>" +
                "<span class='bottom-series-value'>" + info.points[1].valueText + "</span>" +
                "% </div></div></div>"
        };
    }

    customizeLabelText = (info: any) => {
        return info.valueText + "%";
    }
}



