import { AccountService } from './../_services/account.service';
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-curso',
  templateUrl: './curso.component.html',
  styleUrls: ['./curso.component.css']
})
export class CursoComponent implements OnInit {
  baseUrl = 'https://localhost:5001/api/course';
  courses: any;


  constructor(public accountService: AccountService, public http: HttpClient) { }

  ngOnInit(): void {
    this.getCourses();
  }

  getCourses(){
    this.http.get(this.baseUrl).subscribe(response => {
      this.courses = response;
      console.log(this.courses)
    }, error => {
      console.log(error)
    });
  }

}
