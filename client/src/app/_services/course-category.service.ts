import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CourseCategoryService {
  baseUrl = 'https://localhost:5001/api/courseCategories'
  courseCategories: any;

  constructor(public http: HttpClient) { }

  getCourseCategories(){
    this.http.get(this.baseUrl).subscribe(response => {
      this.courseCategories = response;
    }, error => {
      console.log(error)
    })
  }
}
