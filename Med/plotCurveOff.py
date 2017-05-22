# execute this in jupyter notebook to get result

import pandas as pd

import plotly
from plotly.graph_objs import Scatter, Layout, Figure

import plotly.offline as offline

df = pd.read_csv('leftDatas.csv')
df.head()

offline.init_notebook_mode(connected=True)

trace1 = Scatter(x=df['timestamp'], y=df['eyeY'], mode='lines', name='EyeYVariation')

trace2 = Scatter(x=df['timestamp'], y=df['eyeX'], mode='lines', name='EyeXVariation')

layout = Layout(title='Eye position Analysis', plot_bgcolor='rgb(230, 230,230)')

fig = Figure(data=[trace1, trace2], layout=layout)

offline.iplot(fig, show_link=False)