export const MOCK_COMPOSITIONS: orch.Composition[] = [
  {
    id: 11111,
    title: "Symphony 9",
    composer: "Beethoven",
    genre: "Classical",
    uploader: "Zubin Mehta",
  },
  {
    id: 22222,
    title: "Israeli Songs",
    composer: "Israel Cohen",
    genre: "Israeli",
    uploader: "Director 1",
  },
  {
    id: 33333,
    title: "All the things you are",
    composer: "Jerome Kern",
    genre: "Jazz",
    uploader: "Director 2",
  },
  {
    id: 44444,
    title: "temp",
    composer: "temp",
    genre: "Classical",
    uploader: "Director 1",
  },
  {
    id: 55555,
    title: "Harry Potter Medley",
    composer: "John Williams",
    genre: "Movies",
    uploader: "Director 2",
  },
];

export const MOCK_GENRES = Array.from(new Set(MOCK_COMPOSITIONS.map(_ => _.genre)));
