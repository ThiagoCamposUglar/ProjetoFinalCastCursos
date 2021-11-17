import { User } from './../_models/user';
import { AccountService } from './../_services/account.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  errorMessage: any;

  constructor(public accountService: AccountService) { }
  currentUser: any;


  ngOnInit(): void {
    this.accountService.currentUser$.subscribe(user => this.currentUser = user);
  }

  login(){
    this.errorMessage = "";
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error)
      this.errorMessage = error;
      window.alert(this.errorMessage.error);
    });
  }

  logout(){
    this.accountService.logout();
  }
}
