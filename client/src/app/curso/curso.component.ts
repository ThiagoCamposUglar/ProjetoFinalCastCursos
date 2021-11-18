import { CourseCategoryService } from './../_services/course-category.service';
import { AccountService } from './../_services/account.service';
import { Component, OnInit, TemplateRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-curso',
  templateUrl: './curso.component.html',
  styleUrls: ['./curso.component.css']
})
export class CursoComponent implements OnInit {
  baseUrl = 'https://localhost:5001/api/course';
  courses: any;
  modalRef?: BsModalRef;
  public newCourseForm: FormGroup;
  public editCourseForm: FormGroup;
  currentUser: any;
  errorMessage: any;
  succsessMessage: any;
  selectedCourse: any;
  private _listFilter: string = '';
  public filteredCourses: any[];

  public get listFilter(): string {
    return this._listFilter;
  }
  public set listFilter(value: string) {
    this._listFilter = value;
    this.filteredCourses = this.listFilter ? this.filterCourses(this.listFilter): this.courses;
  }

  openModal(template: TemplateRef<any>){
    this.modalRef = this.modalService.show(template);
  }

  constructor(public accountService: AccountService, public http: HttpClient, private modalService: BsModalService, private fb: FormBuilder, public courseCategoryService: CourseCategoryService) {
    this.crateNewCourseForm();
    this.createForm();
   }

  ngOnInit(): void {
    this.getCourses();
    this.courseCategoryService.getCourseCategories();
    this.accountService.currentUser$.subscribe(user => this.currentUser = user);
    console.log(this.courses);
  }

  getCourses(){
    this.http.get(this.baseUrl).subscribe(response => {
      this.courses = response;
      this.filteredCourses = this.courses;
      console.log(this.courses)
    }, error => {
      console.log(error)
    });
  }

  postCourse(){
    this.errorMessage = "";
    console.log(this.newCourseForm.value);
    console.log(this.currentUser.id)
    this.http.post(`${this.baseUrl}/${this.currentUser.id}`, this.newCourseForm.value, {responseType: 'text'}).subscribe(response => {
      window.alert(response);
      this.modalRef?.hide();
      this.newCourseForm.reset();
      this.getCourses();
    }, error => {
      console.log(error);
      this.errorMessage = error;
      window.alert(this.errorMessage.error);
    })
  }

  deleteSelectedCourse(id: number){
    this.errorMessage = "";
    this.http.delete(`${this.baseUrl}/${id}`, {responseType: 'text'}).subscribe(response => {
      window.alert(response);
      this.modalRef?.hide();
      this.getCourses();
    }, error => {
      console.log(error);
      this.errorMessage = error;
      window.alert(this.errorMessage.error);
    })
  }

  putCourse(){
    this.errorMessage = "";
    console.log(this.editCourseForm.value)
    this.http.put(`${this.baseUrl}/${this.selectedCourse.id}`, this.editCourseForm.value, {responseType: 'text'}).subscribe(response => {
      window.alert(response);
      this.modalRef?.hide();
      this.getCourses();
    }, error => {
      this.errorMessage = error;
      window.alert(this.errorMessage.error);
    })
  }

  crateNewCourseForm(){
    this.newCourseForm = this.fb.group({
      courseName:['', Validators.required],
      description:['', Validators.required],
      startDate:['', Validators.required],
      endDate:['', Validators.required],
      studentsQuantity:[],
      courseCategoryId:[]
    })
  }

  createForm(){
    this.editCourseForm = this.fb.group({
      id:['', Validators.required],
      courseName:['', Validators.required],
      description:['', Validators.required],
      startDate:['', Validators.required],
      endDate:['', Validators.required],
      studentsQuantity:[],
      courseCategoryId:[]
    })
  }

  deleteCourseSelect(course: any){
    this.selectedCourse = course;
  }

  editCourseSelect(course: any){
    this.selectedCourse = course;
    const datepipe: DatePipe = new DatePipe("en-US")
    let formattedStartDate = datepipe.transform(this.selectedCourse.startDate, 'yyyy-MM-dd')
    this.selectedCourse.startDate = formattedStartDate;
    let formattedEndDate = datepipe.transform(this.selectedCourse.endDate, 'yyyy-MM-dd')
    this.selectedCourse.endDate = formattedEndDate;
    this.editCourseForm.patchValue(course);
    console.log(this.selectedCourse);
  }

  filterCourses(filterBy: string): any{
    const datepipe: DatePipe = new DatePipe("en-US")
    filterBy = filterBy.toLocaleLowerCase();
    return this.courses.filter((course: {courseName: string; startDate: Date; endDate: Date}) => course.courseName.toLocaleLowerCase().indexOf(filterBy) !== -1 ||
    datepipe.transform(course.startDate, 'dd/MM/yyyy').toLocaleString().indexOf(filterBy) !== -1 ||
    datepipe.transform(course.endDate, 'dd/MM/yyyy').toLocaleString().indexOf(filterBy) !== -1);
  }
}
