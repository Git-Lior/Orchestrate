const MOCK_ROLE_ID_1 = 6151;
const MOCK_ROLE_ID_2 = 5155;

export const MOCK_USER_INFO: orch.group.UserInfo = {
  director: true,
  manager: false,
  roles: [
    {
      id: MOCK_ROLE_ID_1,
      section: "Trombone",
      num: 1,
    },
    {
      id: MOCK_ROLE_ID_2,
      section: "Piano",
    },
  ],
};

export const MOCK_COMPOSITIONS: orch.Composition[] = [
  {
    id: 11111,
    title: "Symphony 9",
    composer: "Beethoven",
    genre: "Classical",
    uploader: "Zubin Mehta",
    roles: MOCK_USER_INFO.roles,
  },
  {
    id: 22222,
    title: "Israeli Songs",
    composer: "Israel Cohen",
    genre: "Israeli",
    uploader: "Director 1",
    roles: MOCK_USER_INFO.roles,
  },
  {
    id: 33333,
    title: "All the things you are",
    composer: "Jerome Kern",
    genre: "Jazz",
    uploader: "Director 2",
    roles: MOCK_USER_INFO.roles,
  },
  {
    id: 44444,
    title: "temp",
    composer: "temp",
    genre: "Classical",
    uploader: "Director 1",
    roles: MOCK_USER_INFO.roles,
  },
  {
    id: 55555,
    title: "Harry Potter Medley",
    composer: "John Williams",
    genre: "Movies",
    uploader: "Director 2",
    roles: MOCK_USER_INFO.roles,
  },
];

export const MOCK_GENRES = Array.from(new Set(MOCK_COMPOSITIONS.map(_ => _.genre)));

export const MOCK_SHEET_MUSIC: Record<string, orch.SheetMusic> = {
  [MOCK_ROLE_ID_2.toString()]: {
    fileUrl:
      "http://waltercosand.com/CosandScores/Composers%20A-D/Cortot,%20Alfred/Chopin-Cortot_Etudes_Op.10(Engl).pdf",
    comments: [
      {
        commentId: 1321312,
        content: "hello world!",
        user: {
          director: true,
          name: "John Williams",
        },
      },
    ],
  },
};
