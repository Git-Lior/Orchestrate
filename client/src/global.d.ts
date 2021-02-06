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

  interface Composition {
    id: number;
    title: string;
    composer: string;
    genre: string;
    uploader: string;
  }

  namespace group {
    interface RouteParams {
      groupId?: string;
      groupPage?: string;
    }

    interface UserInfo {
      manager: boolean;
      director: boolean;
      roles: Role[];
    }

    interface Role {
      section: string;
      num: number;
    }

    interface PageProps {
      user: User;
      userInfo: group.UserInfo;
    }

    interface PageInfo {
      name: string;
      route: string;
      Component: React.ComponentType<group.PageProps>;
    }
  }

  namespace compositions {
    interface RouteParams extends group.RouteParams {
      compositionId?: string;
      roleId?: string;
    }

    interface Query {
      genre: string;
      title: string;
      onlyInConcert: boolean;
    }
  }
}
