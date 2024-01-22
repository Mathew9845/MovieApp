import pandas as pd

print("Starting script...")

print("Loading and merging 'titleakas.tsv' and 'titlebasics.tsv'...")
titleakas_path = 'titleakas.tsv'
titlebasics_path = 'titlebasics.tsv'
titleakas = pd.read_csv(titleakas_path, sep='\t', dtype={'titleId': str}, low_memory=False)
titlebasics = pd.read_csv(titlebasics_path, sep='\t', dtype={'tconst': str}, low_memory=False)
merged_data = pd.merge(titleakas, titlebasics, left_on='titleId', right_on='tconst')
print(f"Merging complete. Rows after merge: {len(merged_data)}")

print("Filtering data based on 'titleType'...")
initial_row_count = len(merged_data)
filtered_data_titleType = merged_data[merged_data['titleType'].isin(['tvMovie', 'movie', 'video'])]
print(f"Rows after filtering by 'titleType': {len(filtered_data_titleType)}. Rows removed: {initial_row_count - len(filtered_data_titleType)}")

print("Merging filtered data with 'titleratings.tsv'...")
title_ratings_path = 'titleratings.tsv'
title_ratings = pd.read_csv(title_ratings_path, sep='\t')
final_merged_data = pd.merge(filtered_data_titleType, title_ratings, left_on='tconst', right_on='tconst')
print(f"Merging complete. Rows after merging with 'titleratings': {len(final_merged_data)}")

print("Applying additional filters...")
initial_row_count = len(final_merged_data)
final_filtered_data = final_merged_data[(final_merged_data['numVotes'] >= 1000) & (final_merged_data['region'] == 'US')]
print(f"Rows after additional filters: {len(final_filtered_data)}. Rows removed: {initial_row_count - len(final_filtered_data)}")

print("Merging with 'titlecrew.tsv'...")
titlecrew_path = 'titlecrew.tsv'
titlecrew = pd.read_csv(titlecrew_path, sep='\t')
final_filtered_data = pd.merge(final_filtered_data, titlecrew, on='tconst', how='left')
print("Merging complete.")

print("Removing duplicates and unwanted columns...")
initial_row_count = len(final_filtered_data)
final_filtered_data = final_filtered_data.drop_duplicates(subset='titleId')
final_filtered_data = final_filtered_data.drop(columns=['isAdult', 'titleType', 'isOriginalTitle', 'types', 'ordering', 'attributes', 'language', 'region', 'endYear'])
final_filtered_data = final_filtered_data.rename(columns={'startYear': 'Year'})
print(f"Rows after cleanup: {len(final_filtered_data)}. Rows removed: {initial_row_count - len(final_filtered_data)}")

print("Loading 'namebasics.tsv' for directors and writers information...")
namebasics_path = 'namebasics.tsv'
namebasics = pd.read_csv(namebasics_path, sep='\t', dtype={'tconst': str})

print("Processing directors information...")
directors_exploded = final_filtered_data.assign(directors=final_filtered_data['directors'].str.split(',')).explode('directors')
merged_directors = pd.merge(directors_exploded, namebasics, left_on='directors', right_on='nconst', how='left')
merged_directors['directorNames'] = merged_directors.groupby(['titleId'])['primaryName'].transform(lambda x: ','.join(x.dropna()))

print("Processing writers information...")
writers_exploded = final_filtered_data.assign(writers=final_filtered_data['writers'].str.split(',')).explode('writers')
merged_writers = pd.merge(writers_exploded, namebasics, left_on='writers', right_on='nconst', how='left')
merged_writers['writerNames'] = merged_writers.groupby(['titleId'])['primaryName'].transform(lambda x: ','.join(x.dropna()))

print("Combining all processed data...")
final_data = pd.merge(final_filtered_data, merged_directors[['titleId', 'directorNames']].drop_duplicates('titleId'), on='titleId', how='left')
final_data = pd.merge(final_data, merged_writers[['titleId', 'writerNames']].drop_duplicates('titleId'), on='titleId', how='left')
print(f"Combined data rows: {len(final_data)}")

print("Saving the final data...")
final_data.to_csv('final_data_with_names.tsv', sep='\t', index=False)

print("Final data with names saved in 'final_data_with_names.tsv'")
print("Script completed.")

