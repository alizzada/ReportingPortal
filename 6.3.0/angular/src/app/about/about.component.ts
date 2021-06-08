import { Component, Injector, ChangeDetectionStrategy } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { GetUsersServiceProxy, ListUser } from '@shared/service-proxies/service-proxies';
import { Observable } from 'rxjs';


@Component({
  templateUrl: './about.component.html',
  animations: [appModuleAnimation()],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers:[GetUsersServiceProxy]
})
export class AboutComponent extends AppComponentBase {
  constructor(injector: Injector,private userService: GetUsersServiceProxy) {
    super(injector);
  }
users:ListUser[];



// ngOnInit(){
//   this.getUsersList();
// }

getUsersList(){
  return this.userService.getListUsers().subscribe(response=>{
   this.users=response;
  },(err)=>{
    console.log(err);
  });
}


}
