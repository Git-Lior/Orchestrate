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

  interface CompositionData {
    title: string;
    composer: string;
    genre: string;
    sheetMusic: SheetMusic[];
  }

  interface Composition extends CompositionData {
    id: number;
    uploader: string;
  }

  interface SheetMusic {
    role: group.Role;
    fileUrl: string;
  }

  interface SheetMusicComment {
    commentId: number;
    user: {
      name: string;
      director: boolean;
    };
    content: string;
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
      id: number;
      section: string;
      num?: number;
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
