const MOCK_ROLE_ID_1 = 6151;
const MOCK_ROLE_ID_2 = 5155;

const MOCK_ROLES: orch.Role[] = [
  {
    id: MOCK_ROLE_ID_1,
    section: "Trombone",
    num: 1,
  },
  {
    id: MOCK_ROLE_ID_2,
    section: "Piano",
  },
];

export const MOCK_USER_INFO: orch.group.UserInfo = {
  director: true,
  manager: false,
  roles: MOCK_ROLES,
};

export const MOCK_COMMENTS: orch.SheetMusicComment[] = [
  {
    commentId: 1321312,
    content: "hello world!",
    user: {
      director: true,
      name: "John Williams",
    },
  },
];
