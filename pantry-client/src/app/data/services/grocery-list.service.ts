import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import KitchenList from '../models/KitchenList';
import { UserLoginService } from 'src/app/shared/services/user-login.service';


@Injectable({
  providedIn: 'root'
})
export default class GroceryListApi {
  public API = environment.baseUrl;
  public KITCHEN_ENDPOINT = `${this.API}/KitchenList`;

  public observableList: BehaviorSubject<Array<KitchenList>>;
  private $activeList: BehaviorSubject<KitchenList> = new BehaviorSubject<KitchenList>(null);

  public activeList: Observable<KitchenList> = this.$activeList.asObservable();


  constructor(private http: HttpClient, private userService: UserLoginService) {
    this.observableList = new BehaviorSubject<Array<KitchenList>>([]);
  }

  public setObservable(lists: Array<KitchenList>) {
    this.observableList.next(lists);
  }

  public setActiveList(list: KitchenList) {
    this.$activeList.next(list);
  }

  public addToObservable(addedList: KitchenList): void {
    let newList = this.observableList.getValue();
    newList.push(addedList);
    this.observableList.next(newList);
  }

  public removeFromObservable(removedList: KitchenList): void {
    let newList = this.observableList.getValue();
    let index = newList.findIndex(k => k.kitchenListId == removedList.kitchenListId);
    newList.splice(index, 1);
    this.observableList.next(newList);
  }

  getAllGroceryLists(): Observable<Array<KitchenList>> {
    return this.http.get<Array<KitchenList>>(this.KITCHEN_ENDPOINT, this.userService.authHeader);
  }

  addList(kitchen: KitchenList): Observable<KitchenList> {
    return this.http.post<KitchenList>(this.KITCHEN_ENDPOINT, kitchen, this.userService.authHeader);
  }

  deleteList(listId: number): Observable<KitchenList> {
    return this.http.delete<KitchenList>(this.KITCHEN_ENDPOINT + "/" + listId.toString(), this.userService.authHeader);
  }
}
