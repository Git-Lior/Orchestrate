declare namespace orch {
  type OptionalId<T> = Omit<T, "id"> & { id?: T["id"] };

  interface Error {
    error?: string;
    errors?: Record<string, string[]>;
  }

  interface UserData {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
  }

  interface User extends UserData {
    isPasswordTemporary?: number;
    token: string;
  }

  interface GroupData {
    id: number;
    name: string;
    manager: orch.UserData;
  }

  interface GroupPayload {
    name: string;
    managerId: number;
  }

  interface Group extends GroupData {
    directors: orch.UserData[];
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
