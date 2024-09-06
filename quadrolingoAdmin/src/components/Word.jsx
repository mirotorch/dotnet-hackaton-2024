const Word = ({ word, wordLang, updateWord, deleteWord }) => {
  return (
    <div>
      <p>
        {word} ({wordLang})
      </p>
      <button onClick={() => updateWord(word)}>Update Word</button>
      <button onClick={() => deleteWord(word)}>Delete Word</button>
    </div>
  );
};

export default Word