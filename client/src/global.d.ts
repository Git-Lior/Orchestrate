declare namespace orch {
  interface User {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
    token: string;
  }

  interface Group {
    id: number;
    name: string;
  }

  interface RouteMatch {
    groupId?: string;
  }
}
