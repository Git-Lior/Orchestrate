declare namespace orch {
  interface User {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
    token: string;
  }

  interface GroupRouteParams {
    groupId?: string;
    groupPage?: string;
  }

  interface CompositionRouteParams extends GroupRouteParams {
    compositionId?: string;
    roleId?: string;
  }

  interface Group {
    id: number;
    name: string;
  }

  interface Composition {
    id: number;
    title: string;
    composer: string;
    genre: string;
    uploader: string;
  }
}
