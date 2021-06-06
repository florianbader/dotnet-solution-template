import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { Todo, TodoService } from './shared/services/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent implements OnInit {
  public todos$!: Observable<Todo[]>;

  constructor(translate: TranslateService, private todoService: TodoService) {
    translate.setDefaultLang('en');
    translate.use('en');
  }

  public ngOnInit(): void {
    this.todos$ = this.todoService.apiTodoGet();
  }
}
