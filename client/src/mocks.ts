export const MOCK_GROUPS: orch.Group[] = [
  {
    id: 123,
    name: "String Trio",
    manager: { id: 111, firstName: "Hello", lastName: "World", email: "helloworld@mail.com" },
    directors: [],
  },
  {
    id: 456,
    name: "Symphonic Orchestra",
    manager: { id: 111, firstName: "Hello", lastName: "World", email: "helloworld@mail.com" },
    directors: [],
  },
  {
    id: 789,
    name: "New Ensemble",
    manager: { id: 111, firstName: "Hello", lastName: "World", email: "helloworld@mail.com" },
    directors: [],
  },
];

const MOCK_ROLE_ID_1 = 6151;
const MOCK_ROLE_ID_2 = 5155;

const MOCK_ROLES: orch.group.Role[] = [
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

export const MOCK_COMPOSITIONS: orch.Composition[] = [
  {
    id: 11111,
    title: "Symphony 9",
    composer: "Beethoven",
    genre: "Classical",
    uploader: "Zubin Mehta",
    sheetMusic: [_getMockSheetMusic(MOCK_ROLES[0])],
  },
  {
    id: 22222,
    title: "Israeli Songs",
    composer: "Israel Cohen",
    genre: "Israeli",
    uploader: "Director 1",
    sheetMusic: MOCK_ROLES.map(_getMockSheetMusic),
  },
  {
    id: 33333,
    title: "All the things you are",
    composer: "Jerome Kern",
    genre: "Jazz",
    uploader: "Director 2",
    sheetMusic: [_getMockSheetMusic(MOCK_ROLES[1])],
  },
  {
    id: 44444,
    title: "temp",
    composer: "temp",
    genre: "Classical",
    uploader: "Director 1",
    sheetMusic: MOCK_ROLES.map(_getMockSheetMusic),
  },
  {
    id: 55555,
    title: "Harry Potter Medley",
    composer: "John Williams",
    genre: "Movies",
    uploader: "Director 2",
    sheetMusic: MOCK_ROLES.map(_getMockSheetMusic),
  },
];

export const MOCK_GENRES = Array.from(new Set(MOCK_COMPOSITIONS.map(_ => _.genre)));

function _getMockSheetMusic(role: orch.group.Role): orch.SheetMusic {
  return {
    fileUrl:
      "http://waltercosand.com/CosandScores/Composers%20A-D/Cortot,%20Alfred/Chopin-Cortot_Etudes_Op.10(Engl).pdf",
    role,
  };
}

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
