declare namespace orch {
  type OptionalId<T> = Omit<T, "id"> & { id?: T["id"] };

  interface Error {
    error?: string;
    errors?: Record<string, string[]>;
  }

  interface PageProps {
    user: orch.User;
    groups?: orch.GroupData[];
    group?: orch.Group;
    userInfo?: orch.group.UserInfo;
    setGroup: (action: React.SetStateAction<orch.Group | undefined>) => void;
  }

  interface UserData {
    id: number;
    email: string;
    firstName: string;
    lastName: string;
  }

  interface User extends UserData {
    isPasswordTemporary: boolean;
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
    roles: RoleWithMembers[];
  }

  interface Role {
    id: number;
    section: string;
    num: number;
  }

  interface RoleWithMembers extends Role {
    members: UserData[];
  }

  interface BasicCompositionData {
    id: number;
    title: string;
  }

  interface CompositionData extends BasicCompositionData {
    composer: string;
    genre: string;
    uploader: orch.UserData;
  }

  interface Composition extends CompositionData {
    roles: Role[];
  }

  interface SheetMusicComment {
    id: number;
    user: UserData;
    createdAt: number;
    updatedAt?: number;
    content: string;
  }

  interface ConcertData {
    id: number;
    location: string;
    date: number;
    description?: string;
  }

  interface Concert extends ConcertData {
    compositions: CompositionData[];
    // for member
    attending?: boolean;
    // for manager
    attendingUsers: UserData[];
    notAttendingUsers: UserData[];
  }

  type UpdateData = orch.CompositionUpdateData | orch.ConcertUpdateData;

  interface CompositionUpdateData {
    date: number;
    id: number;
    title: string;
    uploader: orch.UserData;
  }

  interface ConcertUpdateData {
    date: number;
    attendance: number;
    concert: orch.ConcertData;
  }

  type NotificationData = orch.SheetMusicNotificationData | orch.ConcertNotificationData;

  interface SheetMusicNotificationData {
    date: number;
    groupId: number;
    composition: BasicCompositionData;
    role: Role;
    comments: number;
  }

  interface ConcertNotificationData {
    date: number;
    groupId: number;
    concert: ConcertData;
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
  }

  namespace compositions {
    interface RouteParams extends group.RouteParams {
      compositionId?: string;
      roleId?: string;
    }

    interface Query {
      genre?: string;
      title?: string;
      onlyInConcert?: boolean;
    }
  }
}
